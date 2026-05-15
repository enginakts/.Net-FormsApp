import requests
import sys

def upload_file(filename):
    with open(filename, 'rb') as f:
        response = requests.post('https://uguu.se/upload', files={'files[]': f})
        print(response.json())

upload_file('/tmp/playground-series-s6e5/my_submission_fe.csv')
