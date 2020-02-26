<!--
Comments here
-->

This section is intended as a comprehensive reference for every available API resource, including allowed methods, parameters, headers, status codes, and request and response payloads.

# Authentication and Authorization

The OrderCloud.io API implements OAuth2 combined with our own notion of Roles to control user authentication and authorization at fine-grain level. Be prepared to spend time learning about these concepts and designing the right model for your application. If you over-priviledge your users and attempt to control what they can do via front-end code, you run the risk of technically savvy users circumventing this by making direct API calls.

# Orchestration
Orchestration resources

# Product Catalogs
Products are priced (or unpriced) goods that can be offered to Buyers for placement on Orders. A Product can belong to any number of Catalogs, and a Catalog can be organized into hierarchical Categories to any arbitrary depth. Catalogs are assigned to Buyers, though visibility and pricing may vary by Buyer, User Group, Category, or individual Product. Pricing may include quantity breaks (collectively called a Price Schedule), and Price Schedules may be shared with multiple Products. User-selected Specs, which may include price markups, can be assigned to Products, enabling Buyer configurability at the time of ordering.

# Orders and Fulfillment

The central concept within OrderCloud.io is (not surprisingly) the Order. This is where Buyers, Sellers, Products, Addresses, and just about everything else come together to form business transactions. A feature that is somewhat unique to this area of the API is that you'll use the same set of endpoints regardless of whether you're a Buyer, Seller, or Supplier. What's important is the order *direction*, which is either incoming or outgoing. Order IDs are guaranteed unique within that context. As a seller, this means you can aggregate lists of incoming orders from many Buyers, or outgoing orders to many Suppliers. You may filter a list by Buyer or Supplier, but it's not required to uniquely identify an Order. (Buyers typically only deal with outgoing Orders; Suppliers with incoming.) Buyers add Line Items to Orders by specifying a Product, Quantity, and (optionally) Spec values. Shipping is applied at either the Order level or Line Item level. Shipments are independent of Orders; you can ship a partial Order, or aggregate items from multiple Orders onto a single Shipment.

# Suppliers

Suppliers are a third, optional type of organization used in indirect supply chain scenarios. Like the Seller, they contain Users and Groups. Once established, Products can be configured to auto-forward to Suppliers when ordered. This will automatically create a new PO, notify the Supplier, and track costs and profit margins if configured.

# Seller

At the center of any OrderCloud.io solution is a single Seller organization. It consists directly of Users, Groups, and Addresses (used mainly for shipping calculations), which should be established ahead of Product Catalogs, Buyers, and Suppliers.

# Buyers

Buyers represent real-world legal entities to which the Seller offers Product Catalogs and from which the Seller receives Orders. Buyers contain many of the same things as Sellers (Users, Groups, Addresses) and much more, including Cost Centers, Spending Accounts, and Approval Rules.

Assignments are prevalent concept. Most "things" can be assigned to a User, Group, or entire Buyer organization. Plan ahead, and use Groups to your advantage here. For example, if you assign 100 Addresses directly to 50 Users, you are going to need to create 100 additional assignments every time you add a new User. Or, 50 more assignments for every new Address. With careful planning, Groups can save you from "assignment hell".


