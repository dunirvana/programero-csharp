# Status de pedido com o padrão State

**O problema:** a classe `Order` controla o ciclo de vida do pedido (pendente, pago, enviado, entregue, cancelado, devolvido) com `if` e `switch` em cada método. Quando a devolução entrou como requisito, o método `Return()` foi criado, mas ninguém lembrou de revisar o `Cancel()`. Resultado: um pedido devolvido ainda pode ser cancelado e o cliente recebe o estorno duas vezes. O código está em `src/OrderWorkflow/Before`.

**A solução:** padrão State. Cada status vira uma classe que só implementa as ações permitidas naquele momento; o resto é proibido por padrão na classe base `OrderState`. O pedido apenas delega para o estado atual. O código está em `src/OrderWorkflow/After`.

Os testes de `BeforeOrderTests` documentam o estorno duplo da versão antiga; os de `AfterOrderTests` cobrem as transições e as recusas da versão nova.

## Testes

```bash
docker compose run --rm tests
```

Post: https://programero.blogspot.com/2026/11/order-status-state-pattern.html
