import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
} from '@heroui/react'
import type { MenuItem, MeatOption } from '../models'

interface MeatSelectionModalProps {
  isOpen: boolean
  onClose: () => void
  item: MenuItem | null
  onSelectMeat: (meat: MeatOption) => void
}

export function MeatSelectionModal({ isOpen, onClose, item, onSelectMeat }: MeatSelectionModalProps) {
  return (
    <Modal isOpen={isOpen} onClose={onClose} size="md">
      <ModalContent>
        {item && (
          <>
            <ModalHeader className="flex flex-col gap-1">
              Select Meat for {item.name}
              <p className="text-sm text-default-500 font-normal">
                Choose your preferred protein option
              </p>
            </ModalHeader>
            <ModalBody>
              <div className="space-y-3">
                {item.availableMeats?.map((meat) => (
                  <Button
                    key={meat}
                    variant="flat"
                    size="lg"
                    className="w-full justify-start"
                    onPress={() => onSelectMeat(meat)}
                  >
                    <div className="flex items-center gap-3">
                      <div className="w-3 h-3 rounded-full bg-primary" />
                      <span className="capitalize font-medium">{meat}</span>
                    </div>
                  </Button>
                ))}
              </div>
            </ModalBody>
            <ModalFooter>
              <Button color="danger" variant="light" onPress={onClose}>
                Cancel
              </Button>
            </ModalFooter>
          </>
        )}
      </ModalContent>
    </Modal>
  )
}
