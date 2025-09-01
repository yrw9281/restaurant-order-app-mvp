export type MenuCategory = 
  | 'soups'          // ဟင်းချို（酸湯類）
  | 'salads'         // သုပ်類
  | 'fried'          // ကြော်類
  | 'rice-noodles'   // 主食類
  | 'drinks'         // 飲料
  | 'chinese'        // 中式菜

export type MeatOption = 'chicken' | 'pork' | 'shrimp'

export type MenuItem = {
  number: string
  name: string
  price?: number                   // for single-price items
  description?: string
  category: MenuCategory
  availableMeats?: MeatOption[]
  meatPrices?: Record<MeatOption, number>  // if different prices by meat
}

export const DEFAULT_MENU: MenuItem[] = [
  // ===== ဟင်းချို（酸湯類 / Soups） =====
  {
    number: '101',
    name: '၁၂ မျိုးဟင်းချို',
    category: 'soups',
    availableMeats: ['chicken', 'pork', 'shrimp'],
    meatPrices: { chicken: 7000, pork: 8000, shrimp: 12000 }
  },
  {
    number: '102',
    name: 'ယိုးဒယားဟင်းချို',
    category: 'soups',
    availableMeats: ['chicken', 'pork', 'shrimp'],
    meatPrices: { chicken: 7000, pork: 8000, shrimp: 12000 }
  },
  {
    number: '103',
    name: 'တုံယမ်းဟင်းချို',
    category: 'soups',
    availableMeats: ['chicken', 'pork', 'shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 12000 }
  },
  {
    number: '104',
    name: 'စွန်တန်ဟင်းချို',
    category: 'soups',
    availableMeats: ['chicken', 'pork', 'shrimp'],
    meatPrices: { chicken: 7000, pork: 8000, shrimp: 12000 }
  },
  {
    number: '105',
    name: 'ငုံးဥ၊ ငါးဖယ်လုံးဟင်းချို',
    category: 'soups',
    availableMeats: ['chicken', 'pork', 'shrimp'],
    meatPrices: { chicken: 7000, pork: 8000, shrimp: 120000 }
  },
  { number: '106', name: 'ကြက်ဥဆာဒါး', category: 'soups', price: 5000 },
  { number: '107', name: 'ဘဲဥဟင်းချို', category: 'soups', price: 5000 },
  {
    number: '108',
    name: 'စွပ်ပြုတ်',
    category: 'soups',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 10000, pork: 12000, shrimp: 15000 }
  },

  // ===== သုပ်類（Salads） =====
  { number: '201', name: 'ကြက်သားသုပ်', category: 'salads', price: 8000 },
  { number: '202', name: 'ပြည်ကြီးငါးသုပ်', category: 'salads', price: 12000 },
  { number: '203', name: 'နိုင်တောင်ပံသုပ်', category: 'salads', price: 8000 },
  { number: '204', name: 'ကျောက်ပွင့်သုပ်', category: 'salads', price: 4000 },
  { number: '205', name: 'ပဲကြမ်းသုပ်', category: 'salads', price: 3000 },
  { number: '206', name: 'ကန်စွန်းရွက်သုပ်', category: 'salads', price: 4000 },
  { number: '207', name: 'ဝက်သားသုပ်', category: 'salads', price: 8000 },
  { number: '208', name: 'ပုဇွန်အစိမ်းသုပ်', category: 'salads', price: 15000 },
  { number: '209', name: 'ကြက်ရိုးသုပ်', category: 'salads', price: 4000 },
  { number: '210', name: 'လက်ဖက်သုပ်', category: 'salads', price: 3000 },
  { number: '211', name: 'အဖိုးကြီးသုပ်', category: 'salads', price: 3000 },
  { number: '212', name: 'ရှောက်သီးသုပ်', category: 'salads', price: 3500 },
  { number: '213', name: 'ဝက်ခေါင်းသုပ်', category: 'salads', price: 7000 },
  { number: '214', name: 'ပုဇွန်ကင်သုပ်', category: 'salads', price: 15000 },
  { number: '215', name: 'ဆေးဘဲဥသုပ်', category: 'salads', price: 5000 },
  { number: '216', name: 'လက်ဖက်ပုဇွန်', category: 'salads', price: 3500 },
  { number: '217', name: 'ခရမ်းချဉ်သီးသုပ်', category: 'salads', price: 3000 },

  // ===== ကြော်類（Fried/炒） =====
  { number: '301', name: 'ကြက်ဥမွှေကြော်', category: 'fried', price: 4000 },
  { number: '302', name: 'ကန်စွန်းပလိန်း', category: 'fried', price: 3000 },
  { number: '303', name: 'ပန်းပွင့်ကြော်', category: 'fried', price: 4000 },
  { number: '304', name: 'ငါးမုန့်ကြော်', category: 'fried', price: 4000 },
  { number: '305', name: 'ကန်စွန်ချဉ်စပ်', category: 'fried', price: 4500 },
  {
    number: '306',
    name: 'သီးရွက်စုံ(ကြော်/သုပ်)',
    category: 'fried',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 12000 }
  },
  { number: '307', name: 'အာလူးချောင်းကြော်', category: 'fried', price: 4000 },
  { number: '308', name: 'မုန်ညှင်းကြော်', category: 'fried', price: 4000 },
  { number: '309', name: 'မုန်ညှင်းခရုဆီနံနက်ခင်း', category: 'fried', price: 5000 },
  { number: '310', name: 'မုန့်ဟင်းခါး/ ညှပ်ခေါက်ဆွဲ', category: 'fried', price: 2500 },
  { number: '311', name: 'မုန့်ဖက်သုပ်/ ညှပ်ခေါက်သုပ်', category: 'fried', price: 2500 },

  // ===== 主食類（Rice & Noodles） =====
  { number: '401', name: 'ထမင်း', category: 'rice-noodles', price: 1000 },
  { number: '402', name: 'နန်းကြီးသုပ်', category: 'rice-noodles', price: 4000 },
  { number: '403', name: 'လက်ဖက်ထမင်းသုပ်', category: 'rice-noodles', price: 4000 },
  { number: '404', name: 'ထမင်းသုပ်', category: 'rice-noodles', price: 4000 },
  { number: '405', name: 'ဆီချက်', category: 'rice-noodles', price: 4000 },
  { number: '406', name: 'ထမင်းဆီဆမ်း', category: 'rice-noodles', price: 3500 },
  {
    number: '407',
    name: 'ထမင်းကြော်',
    category: 'rice-noodles',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 7000, pork: 10000, shrimp: 12000 }
  },
  { number: '408', name: 'ပေါင်မုန့်ကြက်ဥကြော်', category: 'rice-noodles', price: 2500 },
  { number: '409', name: 'ပေါင်မုန့်မီးကင်', category: 'rice-noodles', price: 2500 },

  // ===== 飲料（Tea & Drinks） =====
  { number: '501', name: 'လက်ဖက်ရည်', category: 'drinks' },
  { number: '502', name: 'ကော်ဖီ', category: 'drinks' },
  { number: '503', name: 'စီလုံတီး', category: 'drinks' },

  // ===== 中式菜（Chinese Food） =====
  {
    number: '601',
    name: 'ခေါက်ဆွဲကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 7000, pork: 7000, shrimp: 12000 }
  },
  {
    number: '602',
    name: 'ကြာဇံကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 7000, pork: 7000, shrimp: 12000 }
  },
  {
    number: '603',
    name: 'အကြွပ်ကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 15000 }
  },
  {
    number: '604',
    name: 'တရုတ်ထမင်းကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 7000, pork: 70000, shrimp: 15000 }
  },
  {
    number: '605',
    name: 'ချိုချဉ်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 15000 }
  },
  {
    number: '606',
    name: 'ချဉ်စပ်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 15000 }
  },
  { number: '607', name: 'ဆီချက်', category: 'chinese', price: 4000 },
  { number: '608', name: 'ပြည်ကြီးငါး', category: 'chinese' },
  {
    number: '609',
    name: 'အသားလုံးကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 10000, pork: 12000, shrimp: 15000 }
  },
  {
    number: '610',
    name: 'အသားပြားကြော်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 8000, pork: 10000, shrimp: 15000 }
  },
  { number: '611', name: 'ငါးပေါင်းသံပုရာ', category: 'chinese', price: 20000 },
  { number: '612', name: 'ငါးအသား(ချိုချဉ်/ချဉ်စပ်)', category: 'chinese' },
  {
    number: '613',
    name: 'ကုန်းဘောင်',
    category: 'chinese',
    availableMeats: ['chicken','pork','shrimp'],
    meatPrices: { chicken: 10000, pork: 12000, shrimp: 15000 }
  },
  { number: '614', name: 'တန်ပူရာ', category: 'chinese', price: 15000 },
  { number: '615', name: 'ချဉ်မွှေး', category: 'chinese', price: 10000 },
]
