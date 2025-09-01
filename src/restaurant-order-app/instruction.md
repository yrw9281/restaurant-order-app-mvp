# MVP Requirements for Restaurant Order App

This project is a Minimum Viable Product (MVP) for a restaurant order page, designed for restaurant staff. All text in the project should be in English.

**This app is designed for mobile devices.**

## Features

### 1. Menu Management
- Display a list of menu items with the following fields:
	- Item Number (unique identifier)
	- Name (dish name)
	- Price
	- Description (optional)
- Allow waiters to select items and specify:
	- Quantity
	- Notes (special requests, allergies, etc.)
- Support searching and filtering menu items by name or number.

### 2. Order Placement
- Waiters can create a new order by selecting menu items and entering required details.
- Show a summary of the current order before submission.
- Allow editing or removing items from the order before finalizing.
- Submit the order and save it to the order history.

### 3. Order History
- Display a list of all previous orders with:
	- Order ID or Number
	- Timestamp
	- List of ordered items (with quantity, price, notes)
	- Total price
- Allow viewing details of each order.
- Support searching/filtering orders by date, item, or notes.

### 4. User Interface & Experience
- Responsive design for desktop and tablet devices.
- Clear and intuitive navigation between menu and order history.
- Use modern UI components from heroui and TailwindCSS.
- All labels, buttons, and messages in English.

### 5. Technical Requirements
- Use React for building UI components.
- Use Vite for project setup and development.
- Use TailwindCSS for styling.
- Use heroui for UI components.
- Store menu and order data in local state (no backend required for MVP).

---
This MVP should provide a simple, efficient workflow for restaurant staff to place orders and review order history.
