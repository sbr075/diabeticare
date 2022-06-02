## Server for Diabeticare 
This is an test server for the application "Diabeticare" programmed using Python and Flask 
The server offers a RESTful API without any frontend
All programming and testing was done using a virtual enviroment (using GIT BASH for run.sh script)

### How to set up a venv
1. Create the virtual enviroment
> python3 -m venv venv
2. Activate the virtual enviroment
> venv/Scripts/activate
*NB! Use "python3 -m pip install 'library'" to install dependencies inside the virtual enviroment*

### Download Postgresql
Using pgAdmin to manage postgresql server 
1. Go to https://www.enterprisedb.com/downloads/postgres-postgresql-downloads
2. Setup database

### How to migrate database changes
1. Run 'cd src' in your terminal
2. Run 'sh diabeticare/migrate.sh' in your terminal

### Run as HTTPs
1. Run 'flask run --port=8000' in your terminal

### Dependencies 
- Python (>=3.10.2)
- Flask (>=2.02)
- flask-login (>=0.5.0)
- flask-wtf (>=1.0.0)
- flask-migrate(>=1.1.2)
- Flask-SQLAlchemy
- Flask-Bootstrap (>=3.3.7.1)
- Werkzeug (>=2.0)
  - Implements WSGI, the standard Python interface between applications and servers.
- Jinja (>=3.0)
  - A template language that renders the pages your application serves.
- MarkupSafe (>=2.0)
  - It escapes untrusted input when rendering templates to avoid injection attacks.
- ItsDangerous (>=2.0)
  - Securely signs data to ensure its integrity. This is used to protect Flask’s session cookie.
- Click (>=7.1.2)
  - A framework for writing command line applications. It provides the flask command and allows adding custom management commands.
- psycopg2
  - Used for postgresql
- passlib (>=1.7.4)
- python-dotenv (>=0.19.2)
- pyOpenSSL (>=22.0.0)