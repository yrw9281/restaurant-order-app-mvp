import { Button, Card, CardBody, Chip } from '@heroui/react'
import type { MenuItem } from '../models'
import { currency } from '../hooks/utils'

interface MenuItemCardProps {
  item: MenuItem
  onAddToCart: (item: MenuItem) => void
}

export function MenuItemCard({ item, onAddToCart }: MenuItemCardProps) {
  return (
    <Card className="p-0 w-full" isPressable onPress={() => onAddToCart(item)}>
      <CardBody className="p-4 w-full">
        <div className="flex items-start justify-between w-full gap-4">
          <div className="flex-1 flex flex-col min-w-0">
            <div className="w-full">
              <div className="flex items-center gap-2 mb-2">
                <Chip size="sm" variant="flat" color="default">#{item.number}</Chip>
                {item.availableMeats && item.availableMeats.length > 0 && (
                  <Chip size="sm" variant="flat" color="secondary">
                    Customizable
                  </Chip>
                )}
              </div>
              <h3 className="font-semibold text-lg mb-2 w-full">{item.name}</h3>
              {item.description && (
                <p className="text-default-500 text-sm mb-3 w-full leading-relaxed">{item.description}</p>
              )}
              {item.availableMeats && item.availableMeats.length > 0 && (
                <p className="text-sm text-secondary mb-2">
                  Available with: {item.availableMeats.join(', ')}
                </p>
              )}
            </div>
            {item.price ? (
              <div className="text-right">
                <p className="text-xl font-bold text-success">{currency(item.price)}</p>
              </div>
            ) : item.meatPrices ? (
              <div className="text-right">
                <p className="text-sm text-default-500">From</p>
                <p className="text-xl font-bold text-success">
                  {currency(Math.min(...Object.values(item.meatPrices)))}
                </p>
              </div>
            ) : (
              <div className="text-right">
                <p className="text-sm text-default-500">Price on request</p>
              </div>
            )}
          </div>
          <div className="flex-shrink-0 self-center">
            <Button
              color="primary"
              size="lg"
              className="min-w-12 h-12 rounded-full"
              onPress={() => onAddToCart(item)}
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
            </Button>
          </div>
        </div>
      </CardBody>
    </Card>
  )
}
