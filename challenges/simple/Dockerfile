FROM gforth/safe

WORKDIR /flask

COPY simple safe_files/
COPY checker.fs .
COPY setup.fs .

CMD ["gunicorn", "--config", "gunicorn.config.py", "wsgi:app"]
