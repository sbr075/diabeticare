#!/usr/bin/env python3
""" Testing script for backend server """
from http import server
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
        return resp.json()["X-CSRFToken"]

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

        logger.info(headers)
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
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
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
        data = {"username": USERNAME, "value": value, "timestamp": time, "server_id": -1}
        url = self.create_URL("s/bgl/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def bgl_update(self): # TEST 5
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        server_id = input("Enter existing id: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time, "server_id": server_id}
        url = self.create_URL("s/bgl/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

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
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def bgl_delete(self): # TEST 7
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "server_id": server_id}
        url = self.create_URL("s/bgl/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def sleep_add(self): # TEST 8
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "start": time, "stop": time+1000, "server_id": -1}
        url = self.create_URL("s/sleep/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def sleep_update(self): # TEST 9
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "start": time, "stop": time+1000, "server_id": server_id}
        url = self.create_URL("s/sleep/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
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
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def sleep_delete(self): # TEST 11
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "server_id": server_id}
        url = self.create_URL("s/sleep/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def ci_add(self): # TEST 12
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "name": "FoodItem", "timestamp": time, "server_id": -1}
        url = self.create_URL("s/ci/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def ci_update(self): # TEST 13
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        value = input("Enter value: ")
        server_id = input("Enter existing id: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "name": "UpdatedFoodItem", "timestamp": time, "server_id": server_id}
        url = self.create_URL("s/ci/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

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
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def ci_delete(self): # TEST 15
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(("Enter existing id: "))
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "server_id": server_id}
        url = self.create_URL("s/ci/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def mood_add(self): # TEST 16
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        value = input("Enter value: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time, "server_id": -1}
        url = self.create_URL("s/mood/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def mood_update(self): # TEST 17
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        value = input("Enter value: ")
        server_id = input("Enter existing id: ")
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "value": value, "timestamp": time, "server_id": server_id}
        url = self.create_URL("s/mood/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def mood_get(self): # TEST 18
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        minutes = int(input("Minutes: "))
        time = (datetime.datetime.now() - datetime.timedelta(minutes=minutes)).timestamp()
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "timestamp": time}
        url = self.create_URL("s/mood/get")

        resp = self.do_GET(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]

    def mood_delete(self): # TEST 19
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "server_id": server_id}
        url = self.create_URL("s/mood/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def exercise_add(self): # TEST 20
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "name": "ExerciseName", "start": time, "stop": time+1000, "server_id": -1}
        url = self.create_URL("s/exercise/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def exercise_update(self): # TEST 21
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))

        headers = {"X-CSRFToken": CSRF_TOKEN}
        time = datetime.datetime.now().timestamp()
        data = {"username": USERNAME, "name": "UpdatedExerciseName", "start": time, "stop": time+1000, "server_id": server_id}
        url = self.create_URL("s/exercise/set")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def exercise_get(self): # TEST 22
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        minutes = int(input("Minutes: "))
        time = (datetime.datetime.now() - datetime.timedelta(minutes=minutes)).timestamp()
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "timestamp": time}
        url = self.create_URL("s/exercise/get")

        resp = self.do_GET(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]
    
    def exercise_delete(self): # TEST 23
        global CSRF_TOKEN
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return
        
        server_id = int(input("Enter existing id: "))
        
        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": USERNAME, "server_id": server_id}
        url = self.create_URL("s/exercise/del")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")
        
        CSRF_TOKEN = resp.json()["X-CSRFToken"]


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
    "15": testerFuncs.ci_delete,

    # Mood database functions
    "16": testerFuncs.mood_add,
    "17": testerFuncs.mood_update,
    "18": testerFuncs.mood_get,
    "19": testerFuncs.mood_delete,

    # Exercise database functions
    "20": testerFuncs.exercise_add,
    "21": testerFuncs.exercise_update,
    "22": testerFuncs.exercise_get,
    "23": testerFuncs.exercise_delete
}

def main(args):
    for test_nr in args.tests:
        tests[test_nr]()

if __name__ == "__main__":
    parser = arg_parser()
    args   = parser.parse_args()
    
    main(args)