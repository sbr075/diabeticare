#!/usr/bin/env python3
""" Testing script for backend server """
import requests
import argparse
import logging

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

class HTTPMethods():
    def create_URL(self, args=""):
        return f"http://127.0.0.1:5000/{args}"
    
    def do_GET_CSRF(self):
        url = self.create_URL("/get_token")
        resp = session.get(url)
        return resp.cookies.get_dict()["CSRF-TOKEN"]

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
        confirm  = input("Enter password: ")
        email    = input("Enter email: ")

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
        
        global CSRF_TOKEN
        CSRF_TOKEN = resp.cookies.get_dict()["CSRF-TOKEN"]

    def logout(self):
        if not CSRF_TOKEN:
            logger.info(" Not logged in")
            return

        username = input("Enter username: ")

        headers = {"X-CSRFToken": CSRF_TOKEN}
        data = {"username": username}
        url = self.create_URL("u/logout")

        resp = self.do_POST(url, headers, data)
        logger.info(f" Response from server: {resp.json()}")


testerFuncs = Tester()
tests = {
    "1": testerFuncs.register,
    "2": testerFuncs.login,
    "3": testerFuncs.logout
}

def main(args):
    for test_nr in args.tests:
        tests[test_nr]()

if __name__ == "__main__":
    parser = arg_parser()
    args   = parser.parse_args()
    
    main(args)