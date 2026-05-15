import pandas as pd
import numpy as np
import lightgbm as lgb
from sklearn.preprocessing import LabelEncoder
from sklearn.model_selection import train_test_split
import os

def feature_engineering(df):
    df = df.copy()

    # 1. Basic Derived Features
    # Prevent division by zero
    df['Degradation_Per_TyreLife'] = df['Cumulative_Degradation'] / (df['TyreLife'] + 1e-5)
    df['Degradation_Per_Lap'] = df['Cumulative_Degradation'] / (df['LapNumber'] + 1e-5)

    # Non-linear TyreLife
    df['TyreLife_Squared'] = df['TyreLife'] ** 2

    # Race Progress features
    df['Late_Race_Flag'] = (df['RaceProgress'] > 0.85).astype(int)

    # PitStop flag (has pitted already?)
    df['Has_Pitted'] = (df['PitStop'] > 0).astype(int)

    # Position changes
    df['Position_Change_Rate'] = df['Position_Change'] / (df['LapNumber'] + 1e-5)

    # 2. Aggregated Group Features (Frequency/Mean)
    # We will compute these on the whole dataset to avoid train/test mismatch
    # For robust mean, it's better to calculate on train and map, but for simplicity we'll just group by categorical columns

    return df

def main():
    print("Loading data...")
    data_dir = '/tmp/playground-series-s6e5/extracted_files'
    train_path = os.path.join(data_dir, 'train.csv')
    test_path = os.path.join(data_dir, 'test.csv')

    train_df = pd.read_csv(train_path)
    test_df = pd.read_csv(test_path)

    # Target and ID handling
    y = train_df['PitNextLap']
    train_ids = train_df['id']
    test_ids = test_df['id']

    # Keep PitNextLap for training separation later
    train_df = train_df.drop(columns=['id', 'PitNextLap'])
    test_df = test_df.drop(columns=['id'])

    # Combine to apply group features consistently
    train_len = len(train_df)
    full_df = pd.concat([train_df, test_df]).reset_index(drop=True)

    print("Applying Feature Engineering...")
    full_df = feature_engineering(full_df)

    # 3. Target Encoding / Mean Encodings (Using only train data conceptually or whole data for simple counts)
    categorical_cols = ['Driver', 'Compound', 'Race']

    # Frequency Encoding
    for col in categorical_cols:
        full_df[col] = full_df[col].fillna('Unknown')
        freq = full_df[col].value_counts(normalize=True)
        full_df[f'{col}_Freq'] = full_df[col].map(freq)

    # Label Encoding
    for col in categorical_cols:
        le = LabelEncoder()
        full_df[col] = le.fit_transform(full_df[col].astype(str))

    # Split back to train and test
    X_train_full = full_df.iloc[:train_len].copy()
    X_test = full_df.iloc[train_len:].copy()

    print("Splitting data for validation...")
    X_train, X_val, y_train, y_val = train_test_split(X_train_full, y, test_size=0.2, random_state=42, stratify=y)

    print("Training LightGBM model with optimized parameters...")
    # Using more trees, early stopping, and better params
    clf = lgb.LGBMClassifier(
        random_state=42,
        n_estimators=1000,
        learning_rate=0.05,
        max_depth=7,
        num_leaves=64,
        subsample=0.8,
        colsample_bytree=0.8
    )

    # Setup early stopping
    callbacks = [lgb.early_stopping(stopping_rounds=50, verbose=10), lgb.log_evaluation(period=50)]

    clf.fit(
        X_train, y_train,
        eval_set=[(X_val, y_val)],
        callbacks=callbacks
    )

    print("Making predictions...")
    preds = clf.predict(X_test)

    print("Saving submission...")
    submission = pd.DataFrame({
        'id': test_ids,
        'PitNextLap': preds.astype(int)
    })

    sub_out_path = '/tmp/playground-series-s6e5/my_submission_fe.csv'
    submission.to_csv(sub_out_path, index=False)
    print(f"Submission saved to {sub_out_path}")

if __name__ == '__main__':
    main()
