FROM gforth/safe

WORKDIR /flask

COPY commands safe_files/
COPY checker.fs .
COPY setup.fs .
COPY values.py .

CMD ["gunicorn", "--config", "gunicorn.config.py", "wsgi:app"]
