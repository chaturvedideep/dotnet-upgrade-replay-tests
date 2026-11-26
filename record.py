"""Capture responses from a running instance of the OLD system into recordings/*.json.
Usage: python3 record.py http://127.0.0.1:5077
"""
import json, sys, urllib.request, urllib.error, pathlib

base = sys.argv[1].rstrip("/")
cases = [
    ("order-1001", "/orders/1001"),
    ("order-1002", "/orders/1002"),
    ("order-not-found", "/orders/9999"),
    ("export-csv", "/orders/export.csv"),
]
out = pathlib.Path(__file__).parent / "ReplayTests" / "recordings"
out.mkdir(parents=True, exist_ok=True)
for name, path in cases:
    try:
        r = urllib.request.urlopen(base + path)
    except urllib.error.HTTPError as e:
        r = e
    body = r.read().decode("utf-8").replace("\r\n", "\n")
    rec = {
        "name": name, "method": "GET", "path": path, "status": r.status,
        "contentType": r.headers.get_content_type(), "body": body,
    }
    (out / f"{name}.json").write_text(json.dumps(rec, indent=2) + "\n")
    print("recorded", name, r.status)
