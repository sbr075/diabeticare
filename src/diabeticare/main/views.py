from flask import render_template
from flask_wtf.csrf import generate_csrf
from diabeticare.main import bp

@bp.route("/", methods=["GET"])
def home():
    return render_template("home.html")