from flask_wtf import FlaskForm
from wtforms import StringField, PasswordField, SubmitField, BooleanField
from wtforms.validators import InputRequired, EqualTo, Length, ValidationError

from diabeticare.users.models import User
from passlib.hash import sha512_crypt

def validator(form, field):
    username = form.username.data
    password = form.password.data

    user = User.query.filter_by(username=username).first()
    if not user:
        raise ValidationError("Username or password is incorrect.")
    
    if not sha512_crypt.verify(password, user.hash_pwd):
        raise ValidationError("Username or password is incorrect.")


class RegistrationForm(FlaskForm):
    username = StringField("Username", [InputRequired(), Length(min=3, max=20, message="Username must be between 3 and 20 characters long")])
    email = StringField("Email", [InputRequired(), Length(max=80, message="Email cannot be longer than 80 characters")])
    password = PasswordField('Password', [InputRequired(), Length(min=8, max=80, message="Password cannot be less than 8 characters"), EqualTo('confirm', message='Passwords must match')])
    confirm = PasswordField('Confirm password', [InputRequired(), Length(min=8, max=80, message="Password cannot be less than 8 characters"), EqualTo('password', message='Passwords must match')])
    submit = SubmitField("Register")

    def validate_username(self, username):
        user = User.query.filter_by(username=username.data).first()
        if user:
            raise ValidationError("Username already taken.")
    
    def validate_email(self, email):
        user = User.query.filter_by(email=email.data).first()
        if user:
            raise ValidationError("Email already taken.")

class LoginForm(FlaskForm):
    username = StringField("Username", [InputRequired()])
    password = PasswordField("Password", [InputRequired(), validator])
    remember_me = BooleanField("Remember me")
    submit = SubmitField("Login")