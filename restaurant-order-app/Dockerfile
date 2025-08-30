# 使用多階段構建來優化映像大小
# 階段 1: 構建階段
FROM node:18-alpine AS builder

# 設置工作目錄
WORKDIR /app

# 複製 package.json 和 package-lock.json (如果存在)
COPY package*.json ./

# 安裝依賴
RUN npm ci --only=production=false

# 複製源代碼
COPY . .

# 構建應用程式
RUN npm run build

# 階段 2: 生產階段
FROM nginx:alpine AS production

# 複製構建好的文件到 nginx 靜態文件目錄
COPY --from=builder /app/dist /usr/share/nginx/html

# 複製 nginx 配置文件 (如果需要自定義配置)
COPY nginx.conf /etc/nginx/nginx.conf

# 暴露端口
EXPOSE 80

# 啟動 nginx
CMD ["nginx", "-g", "daemon off;"]
