# programero-csharp

Projetos de exemplo em C#/.NET dos posts do blog [PROGRAMERO](https://programero.blogspot.com).

Cada pasta é um projeto independente, ligado a um post. Para rodar qualquer um deles basta ter o Docker instalado:

```bash
cd <pasta-do-projeto>
docker compose up --build
```

E para rodar os testes:

```bash
docker compose run --rm tests
```

## Projetos

| Projeto | Post |
|---|---|
| [2026-11-rate-limiting-minimal-apis](2026-11-rate-limiting-minimal-apis/) | [Rate limiting com Minimal APIs](https://programero.blogspot.com/2026/11/rate-limiting-minimal-apis.html) |
| [2026-11-background-service-channel](2026-11-background-service-channel/) | [Fila em memória com BackgroundService e Channel](https://programero.blogspot.com/2026/11/background-service-channel.html) |

## Problema e solução

| Projeto | Problema | Post |
|---|---|---|
| [2026-11-order-status-state-pattern](2026-11-order-status-state-pattern/) | Status de pedido controlado com `if` espalhado gera estorno duplo | [Status de pedido sem if espalhado: o padrão State](https://programero.blogspot.com/2026/11/order-status-state-pattern.html) |
