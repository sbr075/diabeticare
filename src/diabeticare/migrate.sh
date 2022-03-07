echo "Migrating changes"
flask db init
flask db migrate -m "Migrations"
flask db upgrade