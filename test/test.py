#!/usr/bin/env python3
""" Testing script for backend server """
import requests
import argparse
import logging

import datetime

# Logger
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("TESTER")


def arg_parser():
    parser = argparse.ArgumentParser(prog="tester", description="Tester for diabeticare backend server")

    parser.add_argument("-T", "--tests", type=str, nargs="+", required=True,
            help="test number")
    
    return parser

session = requests.Session()
CSRF_TOKEN = None
USERNAME = None

class HTTPMethods():
    def create_URL(self, args=""):
        return f"http://127.0.0.1:5000/{args}"
    
    def do_GET_CSRF(self):
        url = self.create_URL("/get_token")
        resp = session.get(url)
        return resp.json()["CSRF-Token"]

    def do_GET(self, url: str):
        return session.get(url)

    def do_POST(self, url: str, headers, data):
        return session.post(url, headers=headers, json=data)

    def do_DELETE(self, url: str, data: dict):
        return session.delete(url, data=data)


class Tester(HTTPMethods):
    def register(self):
        username = input("Enter username: ")
        password = input("Enter password: ")
        confirm  = password
        email    = f"{username}@mail.no"

        headers = {"X-CSRFToken": self.do_GET_CSRF()}
        data = {"username": username, "email": email, "password": password, "confirm": confirm}
        url  = self.create_URL("u/register")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
    
    def login(self):
        username = input("Enter username: ")
        password = input("Enter password: ")

        headers = {"X-CSRFToken": self.do_GET_CSRF()}
        data = {"username": username, "password": password}
        url = self.create_URL("u/login")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        global CSRF_TOKEN, USERNAME
        CSRF_TOKEN = resp.json()["CSRF-Token"]
        USERNAME = username

    def logout(self):
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME}
        url = self.create_URL("u/logout")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
    
    def bgl_add(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time}
        url = self.create_URL("s/bgl/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def sleep_add(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "start": time, "stop": time+1000, "timestamp": time}
        url = self.create_URL("s/sleep/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def ci_add(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time}
        url = self.create_URL("s/ci/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]
    
    def bgl_update(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        identifier = input("Enter existing id: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time, "identifier": identifier}
        url = self.create_URL("s/bgl/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def sleep_update(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        identifier = input("Enter existing id: ")

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "start": time, "stop": time+1000, "timestamp": time, "identifier": identifier}
        url = self.create_URL("s/sleep/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def ci_update(self):
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        value = input("Enter value: ")
        identifier = input("Enter existing id: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time, "identifier": identifier}
        url = self.create_URL("s/ci/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

testerFuncs = Tester()
tests = {
    "1": testerFuncs.register,
    "2": testerFuncs.login,
    "3": testerFuncs.logout,
    "4": testerFuncs.bgl_add,
    "5": testerFuncs.sleep_add,
    "6": testerFuncs.ci_add,
    "7": testerFuncs.bgl_update,
    "8": testerFuncs.sleep_update,
    "9": testerFuncs.ci_update
}

def main(args):
    for test_nr in args.tests:
        tests[test_nr]()

if __name__ == "__main__":
    parser = arg_parser()
    args   = parser.parse_args()
    
    main(args)