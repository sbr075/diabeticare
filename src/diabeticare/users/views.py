from flask import render_template, redirect, url_for, request, jsonify
from flask_login import login_user, logout_user, current_user
from passlib.hash import sha512_crypt

from diabeticare import db
from diabeticare.users import bp
from diabeticare.users.forms import RegistrationForm, LoginForm
from diabeticare.users.models import User

# REMOVE
import logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("USER VIEWS")

@bp.route("/login", methods=['GET', 'POST'])
def login():
    if current_user.is_authenticated:
        return redirect(url_for("main.home"))
    
    form = LoginForm()
    if request.method == "POST":
        if form.validate_on_submit():
            user = User.query.filter_by(username=form.username.data).first()
            login_user(user, remember=form.remember_me.data)

            return redirect(url_for("main.home"))
    
    return render_template("login.html", form=form)

@bp.route("/logout", methods=['GET'])
def logout():
    logout_user()
    return redirect(url_for("main.home"))

@bp.route("/register", methods=['GET', 'POST'])
def register():
    if request.method == "POST":
        data = request.get_json()
        reg_form = RegistrationForm(obj=data)

        if reg_form.validate():
            hash_pwd = sha512_crypt.hash(reg_form.password.data)
            user = User(username=reg_form.username.data, email=reg_form.email.data, hash_pwd=hash_pwd)

            db.session.add(user)
            db.session.commit()

            return jsonify({"SUCCESS": "Account successfully created!"})
        
        else:
            return jsonify({"ERROR": reg_form.errors})
    
    return jsonify({"ERROR": "Invalid request"})