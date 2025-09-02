#!/bin/bash

# Restaurant Order API 測試腳本
API_BASE="http://localhost:5248/api"

echo "=== Restaurant Order API 測試 ==="

# 1. 註冊新用戶
echo "1. 註冊新用戶..."
REGISTER_RESPONSE=$(curl -s -X POST "$API_BASE/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "server@restaurant.com",
    "password": "Server123!",
    "displayName": "Server User"
  }')

echo "註冊響應: $REGISTER_RESPONSE"

# 從響應中提取 token
TOKEN=$(echo $REGISTER_RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
echo "Token: $TOKEN"

# 2. 獲取今日選單
echo -e "\n2. 獲取今日選單..."
curl -s -X GET "$API_BASE/menu/today" | jq '.'

# 3. 創建新訂單
echo -e "\n3. 創建新訂單..."
ORDER_RESPONSE=$(curl -s -X POST "$API_BASE/orders" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "type": "DineIn",
    "partySize": 4,
    "tableNo": "T001"
  }')

echo "訂單響應: $ORDER_RESPONSE"

# 從響應中提取訂單 ID
ORDER_ID=$(echo $ORDER_RESPONSE | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "訂單 ID: $ORDER_ID"

# 4. 獲取選單項目來添加到訂單
echo -e "\n4. 獲取選單項目..."
MENU_ITEMS=$(curl -s -X GET "$API_BASE/menu/items" \
  -H "Authorization: Bearer $TOKEN")

echo "選單項目: $MENU_ITEMS"

# 5. 添加訂單項目（使用第一個選單項目）
FIRST_ITEM_ID=$(echo $MENU_ITEMS | jq -r '.[0].id')
echo -e "\n5. 添加訂單項目 (ID: $FIRST_ITEM_ID)..."

curl -s -X POST "$API_BASE/orders/$ORDER_ID/items" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"menuItemId\": \"$FIRST_ITEM_ID\",
    \"quantity\": 2,
    \"notes\": \"No onions please\"
  }" | jq '.'

# 6. 提交訂單
echo -e "\n6. 提交訂單..."
curl -s -X POST "$API_BASE/orders/$ORDER_ID/submit" \
  -H "Authorization: Bearer $TOKEN" | jq '.'

# 7. 獲取所有訂單
echo -e "\n7. 獲取所有訂單..."
curl -s -X GET "$API_BASE/orders" \
  -H "Authorization: Bearer $TOKEN" | jq '.'

echo -e "\n=== 測試完成 ==="
