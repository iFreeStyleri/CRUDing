# CRUDing
Пет-проект, показывающий реализацию "монолита" на языке C#, с использованием ASP.NET, Redis и PostgreSQL, а также реализует JWT-авторизацию.
## Instalation
```
git clone https://github.com/iFreeStyleri/CRUDing && cd /CRUDing && docker compose up -d
```
Далее развёртываются три контейнера: Redis, PostgreSQL и CRUDing.API.
|Наименование|Внешний порт по умолчанию|
|------------|-------------------------|
|Redis|6380|
|PostgreSQL|5430|
|CRUDing.API|5280|
## User Controller
### GET `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|page|int| page >= 0|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|Data: >products|array| Массив продуктов в корзине|
|Data: >products[0]: id|int| id добавленного промежуточного элемента|
|Data: >products[1]: productId|int| id продукта|
|Data: >products[2]: productName|string| наименование продукта|
|Data: >products[3]: cartId|int| id корзины|
|Data: >products[4]: count|int| n > 0, количество продукта в корзине|
|Data: >products[5]: >cost[0]: currency|string|Валюта|
|Data: >products[5]: >cost[0]: value|decimal|Стоимость|
|Data: totalCost: >cost[0]: currency|string|Валюта|
|Data: totalCost: >cost[1]: value|decimal|Общая стоимость корзины|
|message|string?|Сообщение об ошибке|

Возвращает n элементов в корзине пользователя

### POST `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|productId|int|id продукта|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|message|string?|Сообщение об ошибке|

Добавляет продукт в корзину

### DELETE `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|productId|int|id продукта|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|message|string?|Сообщение об ошибке|

Удаляет товар из корзины пользователя
