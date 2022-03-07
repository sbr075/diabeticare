from flask_wtf.csrf import generate_csrf

from diabeticare import create_app

app = create_app()

@app.after_request
def set_csrf_token(response):
    response.set_cookie("CSRF-TOKEN", generate_csrf())
    return response