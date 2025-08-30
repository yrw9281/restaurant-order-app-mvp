import type { Order } from '../models'

// Generate order ID in format yyyyMMdd0001
export function generateOrderId(): string {
  const today = new Date()
  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')
  const datePrefix = `${year}${month}${day}`
  
  // Get existing orders for today
  const existingOrders = JSON.parse(localStorage.getItem('orders') || '[]') as Order[]
  const todayOrders = existingOrders.filter(order => order.id.startsWith(datePrefix))
  const nextNumber = String(todayOrders.length + 1).padStart(4, '0')
  
  return `${datePrefix}${nextNumber}`
}

// Get price for a menu item with optional meat selection
export function getItemPrice(item: { price?: number; meatPrices?: Record<string, number> }, meat?: string): number {
  if (item.meatPrices && meat) {
    return item.meatPrices[meat]
  }
  return item.price || 0
}

export function currency(n: number) {
  return new Intl.NumberFormat('en-US', { 
    style: 'currency', 
    currency: 'MMK',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(n)
}
