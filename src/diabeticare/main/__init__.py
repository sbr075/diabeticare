from flask import Blueprint

bp = Blueprint('main', __name__, template_folder="templates")

from diabeticare.main import views