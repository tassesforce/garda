#!/bin/sh

echo "Running CREATE_TABLES.sql"
psql garda -f /usr/local/bin/CREATE_TABLES.sql
echo "Done running CREATE_TABLES.sql"