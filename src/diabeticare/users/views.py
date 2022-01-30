from flask import render_template, redirect, url_for, request, flash
from flask_login import login_user, logout_user, current_user
from passlib.hash import sha512_crypt

from diabeticare import db
from diabeticare.users import bp
from diabeticare.users.forms import RegistrationForm, LoginForm
from diabeticare.users.models import User

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
    if current_user.is_authenticated:
        return redirect(url_for("main.home"))
    
    form = RegistrationForm()
    if request.method == "POST":
        if form.validate_on_submit():
            hash_pwd = sha512_crypt.hash(form.password.data)
            user = User(username=form.username.data, email=form.email.data, hash_pwd=hash_pwd)

            db.session.add(user)
            db.session.commit()

            flash(f"Successfully created user {form.username.data}!")
            return redirect(url_for("users.login"))
    
    return render_template("registration.html", form=form)