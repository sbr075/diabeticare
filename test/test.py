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

    def do_GET(self, url: str, headers, data):
        return session.get(url, headers=headers, json=data)

    def do_POST(self, url: str, headers, data):
        return session.post(url, headers=headers, json=data)


class Tester(HTTPMethods):
    def register(self): # TEST 1
        username = input("Enter username: ")
        password = input("Enter password: ")
        confirm  = password
        email    = f"{username}@mail.no"

        headers = {"X-CSRFToken": self.do_GET_CSRF()}
        data = {"username": username, "email": email, "password": password, "confirm": confirm}
        url  = self.create_URL("u/register")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
    
    def login(self): # TEST 2
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

    def logout(self): # TEST 3
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME}
        url = self.create_URL("u/logout")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
    
    def bgl_add(self): # TEST 4
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
    
    def bgl_update(self): # TEST 5
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

    def bgl_get(self): # TEST 6
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        minutes = int(input("Minutes: "))
        time = (datetime.datetime.now() - datetime.timedelta(minutes=minutes)).timestamp()
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "timestamp": time}
        url = self.create_URL("s/bgl/get")

        resp = self.do_GET(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def bgl_delete(self): # TEST 7
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        identifiers = [int(i) for i in input("Enter existing id: ").split(" ")]
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "identifiers": identifiers}
        url = self.create_URL("s/bgl/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def sleep_add(self): # TEST 8
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
    
    def sleep_update(self): # TEST 9
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
    
    def sleep_get(self): # TEST 10
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        minutes = int(input("Minutes: "))
        time = (datetime.datetime.now() - datetime.timedelta(minutes=minutes)).timestamp()
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "timestamp": time}
        url = self.create_URL("s/sleep/get")

        resp = self.do_GET(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]
    
    def sleep_delete(self): # TEST 11
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        identifiers = [int(i) for i in input("Enter existing id: ").split(" ")]
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "identifiers": identifiers}
        url = self.create_URL("s/sleep/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def ci_add(self): # TEST 12
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

    def ci_update(self): # TEST 13
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

    def ci_get(self): # TEST 14
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        minutes = int(input("Minutes: "))
        time = (datetime.datetime.now() - datetime.timedelta(minutes=minutes)).timestamp()
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "timestamp": time}
        url = self.create_URL("s/ci/get")

        resp = self.do_GET(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]

    def ci_delete(self): # TEST 15
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        identifiers = [int(i) for i in input("Enter existing id: ").split(" ")]
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "identifiers": identifiers}
        url = self.create_URL("s/ci/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["CSRF-Token"]


testerFuncs = Tester()
tests = {
    # Authentication
    "1":  testerFuncs.register,
    "2":  testerFuncs.login,
    "3":  testerFuncs.logout,

    # BGL database functions
    "4":  testerFuncs.bgl_add,
    "5":  testerFuncs.bgl_update,
    "6":  testerFuncs.bgl_get,
    "7":  testerFuncs.bgl_delete,

    # Sleep database functions
    "8":  testerFuncs.sleep_add,
    "9":  testerFuncs.sleep_update,
    "10": testerFuncs.sleep_get,
    "11": testerFuncs.sleep_delete,

    # CI database functions
    "12": testerFuncs.ci_add,
    "13": testerFuncs.ci_update,
    "14": testerFuncs.ci_get,
    "15": testerFuncs.ci_delete
}

def main(args):
    for test_nr in args.tests:
        tests[test_nr]()

if __name__ == "__main__":
    parser = arg_parser()
    args   = parser.parse_args()
    
    main(args)