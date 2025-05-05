import sqlite3
import pandas as pd
import random
from faker import Faker

fake = Faker()
db_path = 'eav.db'

conn = sqlite3.connect(db_path)
cur = conn.cursor()

cur.executescript("""


CREATE TABLE IF NOT EXISTS Products (
    ProductId INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    Category TEXT NOT NULL,
    BasePrice DECIMAL NOT NULL,
    ReleaseYear INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS CustomerPurchases (
    CustomerId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    PricePaid DECIMAL NOT NULL,
    PurchasedOn TEXT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES MasterEntities(EntityId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
""")
conn.commit()

# Fixing the product code
print("[INFO] Seeding products...")
categories = ['smartphone', 'tablet', 'laptop', 'wearable', 'accessory']
products = [{
    'ProductId': i,
    'Name': f"{fake.word().capitalize()} {i}",
    'Category': random.choice(categories),
    'BasePrice': round(random.uniform(99.99, 1999.99), 2),
    'ReleaseYear': random.randint(2015, 2024)
} for i in range(1, 101)]
pd.DataFrame(products).to_sql("Products", conn, if_exists='replace', index=False)

# Seed purchases (1.5M rows)
print("[INFO] Seeding purchases...")
customer_ids = list(range(1, 1_000_001))
purchases = []
for _ in range(1_500_000):
    cid = random.choice(customer_ids)
    prod = random.choice(products)
    price = round(prod['BasePrice'] * random.uniform(0.85, 1.15), 2)
    date = fake.date_between(start_date='-5y', end_date='today').isoformat()
    purchases.append((cid, prod['ProductId'], price, date))

pd.DataFrame(purchases, columns=['CustomerId', 'ProductId', 'PricePaid', 'PurchasedOn'])\
  .to_sql("CustomerPurchases", conn, if_exists='append', index=False)

conn.commit()
conn.close()

# Number of entities
NUM_ENTITITES = 1