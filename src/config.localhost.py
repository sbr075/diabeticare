"""
From: https://flask.palletsprojects.com/en/2.0.x/config/
- "While it is possible to set ENV and DEBUG in your config or code, 
   this is strongly discouraged. They can’t be read early by the flask command"

Debug is therefor set as an ENVVAR in run.sh
To disable debugging just comment the line out
"""

import os
from pathlib import Path
from dotenv import load_dotenv

# Load BASE_DIR path, and set envvars
BASE_DIR = Path(__file__).resolve().parent.parent
load_dotenv(os.path.join(BASE_DIR, ".env"))

class Config(object):
    # Required for CSRF
    SECRET_KEY  = "2c0463b38c0a511aec708e19ebe103b237854aca4c950b647a416872e0c11340"
    WTF_CSRF_SECRET_KEY = "2asjdnmjan6324234u2baksdm23k42u36523n23ni2aøsdkop234l23o26ad"

    # Database
    databases = {
        "default": {
            "HOST": "localhost",
            "PORT": "5432",
            "NAME": "postgres",
            "USER": "postgres",
            "PASS": "I5Kw3jesAWDO2NsiMAyt2g"
        }
    }

    selected_db = "default"
    DB_HOST = databases[selected_db]["HOST"]
    DB_PORT = databases[selected_db]["PORT"]
    DB_NAME = databases[selected_db]["NAME"]
    DB_USER = databases[selected_db]["USER"]
    DB_PASS = databases[selected_db]["PASS"]

    SQLALCHEMY_DATABASE_URI = f"postgresql://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}"
    SQLALCHEMY_TRACK_MODIFICATIONS = False