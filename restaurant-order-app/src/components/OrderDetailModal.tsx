import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  Divider,
} from '@heroui/react'
import type { Order } from '../models'
import { currency } from '../hooks/utils'

interface OrderDetailModalProps {
  isOpen: boolean
  onClose: () => void
  order: Order | null
}

export function OrderDetailModal({ isOpen, onClose, order }: OrderDetailModalProps) {
  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalContent>
        {order && (
          <>
            <ModalHeader className="flex flex-col gap-1">
              Order #{order.id}
              <p className="text-sm text-default-500 font-normal">
                {new Date(order.timestamp).toLocaleString()}
              </p>
            </ModalHeader>
            <ModalBody>
              <div className="space-y-3">
                {order.items.map((item, idx) => (
                  <div key={idx} className="flex justify-between items-start p-3 bg-content2 rounded-lg">
                    <div className="flex-1">
                      <h4 className="font-semibold">{item.name}</h4>
                      <p className="text-sm text-default-500">#{item.number}</p>
                      {item.selectedMeat && (
                        <p className="text-sm text-secondary">With {item.selectedMeat}</p>
                      )}
                      <p className="text-sm">{currency(item.unitPrice)} × {item.quantity}</p>
                      {item.notes && (
                        <p className="text-sm text-default-600 mt-1 italic">"{item.notes}"</p>
                      )}
                    </div>
                    <span className="font-semibold">{currency(item.unitPrice * item.quantity)}</span>
                  </div>
                ))}
              </div>
              <Divider />
              <div className="flex justify-between items-center">
                <span className="text-xl font-bold">Total</span>
                <span className="text-xl font-bold">{currency(order.total)}</span>
              </div>
            </ModalBody>
            <ModalFooter>
              <Button color="primary" variant="light" onPress={onClose}>
                Close
              </Button>
            </ModalFooter>
          </>
        )}
      </ModalContent>
    </Modal>
  )
}
