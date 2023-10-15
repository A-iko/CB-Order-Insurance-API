# 🟠 CB-Order-Insurance-API 🔵
At Coolblue, we want to be able to insure the products that we sell to a customer, so that we get money back in case the product gets lost or damaged before reaching the customers. For that, we need a REST API that is going to be used by Coolblue webshop. 

# Warning
The current setup does not automatically include the requested surcharge rules. These can be added through the Swagger UI.

# Requirements
1. Make sure the product API is running and available. The address can be configured in the `appsettings.json` file.
Note: If you want to run the unit tests, you might have to turn off the product API because they both use the same port.

# Installation
1. Clone the repository
2. Open the solution in Visual Studio
3. Run the project, it will open a browser window with the Swagger UI
4. That's all!

# Usage
The Swagger UI contains explanation for each endpoint.

# Design Decisions
Design decisions are documented in the (closed) issues here on Github, this way they're easy to find back when using git blame.

# Further development
- [ ] Optimize the fetching of products, we currently have to fetch them one by one because the product api does not support fetching multiple products at once.
- [ ] Create interfaces for the repositories and services, so that we can easily switch between different implementations.
- [ ] Add github actions for building the .net project and creating artifacts when tagged
