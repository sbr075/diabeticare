"""
Guide
> https://flask.palletsprojects.com/en/2.0.x/quickstart/

Enable debug mode
> set FLASK_ENV=development

escape  - prevent injection attacks
url_for - quick routing to URLs instead of hardcoding
"""

from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from flask_bootstrap import Bootstrap
from flask_migrate import Migrate
from flask_login import LoginManager
from flask_wtf.csrf import CSRFProtect
from config import Config

db = SQLAlchemy()
migrate = Migrate()
csrf = CSRFProtect()
login = LoginManager()
login.login_view = "users.login"

def create_app(config_class=Config):
    app = Flask(__name__)
    app.config.from_object(config_class)

    Bootstrap(app)

    db.init_app(app)
    migrate.init_app(app, db)
    csrf.init_app(app)
    login.init_app(app)

    # Blueprints here
    from diabeticare.users import bp as login_app
    app.register_blueprint(login_app, url_prefix="/u")

    from diabeticare.main import bp as main_app
    app.register_blueprint(main_app)

    return app