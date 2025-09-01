import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  ScrollShadow,
  Card,
  CardBody,
  Textarea,
} from '@heroui/react'
import type { OrderLine } from '../models'
import { currency } from '../hooks/utils'

interface CartModalProps {
  isOpen: boolean
  onClose: () => void
  cart: OrderLine[]
  onUpdateQty: (number: string, qty: number) => void
  onUpdateNotes: (number: string, notes: string) => void
  onRemoveLine: (number: string) => void
  onClearCart: () => void
  onSubmitOrder: () => void
}

export function CartModal({
  isOpen,
  onClose,
  cart,
  onUpdateQty,
  onUpdateNotes,
  onRemoveLine,
  onClearCart,
  onSubmitOrder,
}: CartModalProps) {
  const cartTotal = cart.reduce((s, l) => s + l.unitPrice * l.quantity, 0)

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="full" placement="bottom">
      <ModalContent>
        <ModalHeader className="flex flex-col gap-1">
          Current Order
        </ModalHeader>
        <ModalBody>
          <ScrollShadow className="max-h-96">
            {cart.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-default-500">No items in cart</p>
              </div>
            ) : (
              <div className="space-y-4">
                {cart.map(line => (
                  <Card key={line.number} className="p-0">
                    <CardBody className="p-4">
                      <div className="flex items-start justify-between gap-3 mb-3">
                        <div className="flex-1">
                          <h4 className="font-semibold">{line.name}</h4>
                          <p className="text-sm text-default-500">#{line.number}</p>
                          {line.selectedMeat && (
                            <p className="text-sm text-secondary">With {line.selectedMeat}</p>
                          )}
                          <p className="text-sm text-default-500">{currency(line.unitPrice)} each</p>
                        </div>
                        <div className="flex items-center gap-2">
                          <Button
                            size="sm"
                            variant="flat"
                            onPress={() => onUpdateQty(line.number, line.quantity - 1)}
                          >
                            -
                          </Button>
                          <span className="w-8 text-center font-semibold">{line.quantity}</span>
                          <Button
                            size="sm"
                            variant="flat"
                            onPress={() => onUpdateQty(line.number, line.quantity + 1)}
                          >
                            +
                          </Button>
                        </div>
                      </div>
                      <Textarea
                        label="Notes"
                        size="sm"
                        placeholder="Special requests..."
                        value={line.notes || ''}
                        onValueChange={(v) => onUpdateNotes(line.number, v)}
                        className="mb-2"
                      />
                      <div className="flex items-center justify-between">
                        <span className="text-sm font-semibold">
                          Line total: {currency(line.unitPrice * line.quantity)}
                        </span>
                        <Button
                          size="sm"
                          color="danger"
                          variant="light"
                          onPress={() => onRemoveLine(line.number)}
                        >
                          Remove
                        </Button>
                      </div>
                    </CardBody>
                  </Card>
                ))}
              </div>
            )}
          </ScrollShadow>
        </ModalBody>
        <ModalFooter className="flex flex-col gap-3">
          {cart.length > 0 && (
            <>
              <div className="flex items-center justify-between w-full">
                <span className="text-lg font-bold">Total: {currency(cartTotal)}</span>
                <Button size="sm" variant="light" color="danger" onPress={onClearCart}>
                  Clear Cart
                </Button>
              </div>
              <Button color="success" size="lg" className="w-full" onPress={onSubmitOrder}>
                Submit Order
              </Button>
            </>
          )}
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}
