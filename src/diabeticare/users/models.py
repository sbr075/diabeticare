from flask_login import UserMixin
from diabeticare import login, db
import datetime

class User(UserMixin, db.Model):
    __tablename__ = "user"
    
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(25), unique=True, nullable=False)
    email = db.Column(db.String(80), unique=True, nullable=False)
    hash_pwd = db.Column(db.String(), nullable=False)

    cookie = db.Column(db.String())
    timer  = db.Column(db.DateTime(), default=datetime.datetime.utcnow())

@login.user_loader
def load_user(id):
    return User.query.get(int(id))