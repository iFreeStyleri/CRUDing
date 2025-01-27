# CRUDing
Пет-проект, показывающий реализацию "монолита" на языке C#, с использованием ASP.NET, Redis и PostgreSQL, а также реализует JWT-авторизацию.
## Instalation
```
git clone https://github.com/iFreeStyleri/CRUDing && cd /CRUDing && docker compose up -d
```
Далее развёртываются три контейнера: Redis, PostgreSQL и CRUDing.API.
|Наименование|Внешний порт по умолчанию|
|------------|-------------------------|
|Redis|`6380`|
|PostgreSQL|`5430`|
|CRUDing.API|`5280`|
## User Controller
### GET `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|page|int|`page >= 0`|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|data: >products|array| Массив продуктов в корзине|
|data: >products[0]: `id`|int| `id` добавленного промежуточного элемента|
|data: >products[1]: `productId`|int| `id` продукта|
|data: >products[2]: `productName`|string| наименование продукта|
|data: >products[3]: `cartId`|int| `id` корзины|
|data: >products[4]: `count`|int| `count > 0`, количество продукта в корзине|
|data: >products[5]: >cost[0]: `currency`|string|Валюта|
|data: >products[5]: >cost[0]: `value`|decimal|Стоимость|
|data: totalCost: >cost[0]: `currency`|string|Валюта|
|data: totalCost: >cost[1]: `value`|decimal|Общая стоимость корзины|
|message|string?|Сообщение об ошибке|

Возвращает `n элементов` в корзине пользователя

### POST `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|productId|int|`id` продукта|
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
|productId|int|`id` продукта|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|message|string?|Сообщение об ошибке|

Удаляет товар из корзины пользователя

### PATCH `/api/carts`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|productId|int|`id` продукта|
|count|int|кол-во товара в корзине, `count > 0`|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|message|string?|Сообщение об ошибке|

Изменяет количество продукта в корзине пользователя

### DELETE `/api/carts`
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|message|string?|Сообщение об ошибке|

Очищает корзину пользователя
## Category Controller
### GET `/api/categories`
#### HTTP Requests
|Parameter|Type|Description|
|----|-----|-----------|
|id|int?|`id` категории|
|page|int?|номер страницы с категориями|
#### Response Parameters
|Parameter|Type|Description|
|----|-----|-----------|
|code|int| Response status code|
|Data|array| Массив продуктов в корзине|
|>data[0]: `id`|int| `id` категории|
|>data[1]: `name`|string| наименование продукта|
|message|string?|Сообщение об ошибке|

Получение категории по `id` или номеру страницы

To be continue...


