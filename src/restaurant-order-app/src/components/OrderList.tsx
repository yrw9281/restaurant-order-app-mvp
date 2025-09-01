import { Card, CardBody, Chip, Button } from '@heroui/react'
import type { Order } from '../models'
import { currency } from '../hooks/utils'

interface OrderListProps {
  orders: Order[]
  onOrderClick: (order: Order) => void
}

export function OrderList({ orders, onOrderClick }: OrderListProps) {
  if (orders.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-default-500">No orders found</p>
      </div>
    )
  }

  return (
    <div className="space-y-3">
      {orders.map(order => (
        <Card key={order.id} className="p-0 w-full" isPressable onPress={() => onOrderClick(order)}>
          <CardBody className="p-4 w-full">
            <div className="flex items-start justify-between w-full gap-4">
              <div className="flex-1 flex flex-col min-w-0">
                <div className="w-full">
                  <div className="flex items-center gap-2 mb-2">
                    <Chip size="sm" variant="flat" color="primary">#{order.id}</Chip>
                    <Chip size="sm" variant="flat" color="default">
                      {order.items.length} items
                    </Chip>
                  </div>
                  <h3 className="font-semibold text-lg mb-2 w-full">
                    Order from {new Date(order.timestamp).toLocaleDateString()}
                  </h3>
                  <p className="text-default-500 text-sm mb-3 w-full leading-relaxed">
                    {new Date(order.timestamp).toLocaleTimeString()}
                  </p>
                  <div className="space-y-1 mb-3">
                    {order.items.slice(0, 2).map((item, idx) => (
                      <p key={idx} className="text-sm">
                        <span className="font-medium">{item.name}</span> x{item.quantity}
                      </p>
                    ))}
                    {order.items.length > 2 && (
                      <p className="text-sm text-default-500">+{order.items.length - 2} more items</p>
                    )}
                  </div>
                </div>
                <p className="text-xl font-bold text-success">{currency(order.total)}</p>
              </div>
              <div className="flex-shrink-0 self-center">
                <Button
                  color="primary"
                  size="lg"
                  className="min-w-12 h-12 rounded-full"
                  onPress={() => onOrderClick(order)}
                >
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                  </svg>
                </Button>
              </div>
            </div>
          </CardBody>
        </Card>
      ))}
    </div>
  )
}
