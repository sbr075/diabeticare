from diabeticare import db

import datetime

class BGL(db.Model):
    __tablename__ = "bgl"

    id = db.Column(db.Integer, primary_key=True)
    user = db.Column(db.Integer, db.ForeignKey("user.id", lazy=True), nullable=False)

    measurement = db.Column(db.Float(), nullable=False)
    server_date = db.Column(db.DateTime())
    user_date   = db.Column(db.DateTime())
    note = db.Column(db.String(256), nullable=True)

class Sleep(db.Model):
    __tablename__ = "sleep"

    id = db.Column(db.Integer, primary_key=True)
    user = db.Column(db.Integer, db.ForeignKey("user.id", lazy=True), nullable=False)

    user_start = db.Column(db.DateTime())
    user_stop  = db.Column(db.DateTime())
    server_start = db.Column(db.DateTime())
    server_stop  = db.Column(db.DateTime())
    note  = db.Column(db.String(256), nullable=True)

class CI(db.Model):
    __tablename__ = "carbohydrate intake"

    id = db.Column(db.Integer, primary_key=True)
    user = db.Column(db.Integer, db.ForeignKey("user.id", lazy=True), nullable=False)

    carbohydrates = db.Column(db.Integer(), nullable=False)
    server_date = db.Column(db.DateTime())
    user_date   = db.Column(db.DateTime())
    note = db.Column(db.String(256), nullable=True)