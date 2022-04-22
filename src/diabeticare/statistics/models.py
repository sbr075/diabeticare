from diabeticare import db, ma

class BGL(db.Model):
    __tablename__ = "bgl"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    measurement = db.Column(db.Float(), nullable=False)
    date = db.Column(db.Integer, nullable=False)
    note = db.Column(db.String(256), nullable=True)


class BGLSchema(ma.SQLAlchemySchema):
    class Meta:
        model = BGL
    
    measurement = ma.auto_field()
    date = ma.auto_field()
    note = ma.auto_field()


class Sleep(db.Model):
    __tablename__ = "sleep"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    start = db.Column(db.Integer, nullable=False)
    stop  = db.Column(db.Integer, nullable=False)
    note  = db.Column(db.String(256), nullable=True)


class SleepSchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = Sleep

    start = ma.auto_field()
    stop = ma.auto_field()
    note = ma.auto_field()


class CI(db.Model):
    __tablename__ = "carbohydrate intake"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    carbohydrates = db.Column(db.Integer, nullable=False)
    date = db.Column(db.Integer, nullable=False)
    note = db.Column(db.String(256), nullable=True)


class CISchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = CI