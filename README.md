# IdempotencyExample

Example project created for the demonstration of Idempotency in REST API's in TechTalks.

In repository, productList represents a table without unique key. productDictionary represent a table with unique key.  

Stage 1 represents a POST action to insert data to Product table which does not have a unique key.
Stage 2 represents a POST action to insert data to Product table which does have a unique key and gives error when trying to insert data with a key already presented.
Stage 3 represents a POST action to insert data to Product table which has a unique key and returns data when trying to insert data with a key already presented, from DB.
Stage 4 represents a POST action to insert data to Product table which has a unique key and returns data when trying to insert data with a key already presented, from cache. After cache expired, it works like stage 3.  
