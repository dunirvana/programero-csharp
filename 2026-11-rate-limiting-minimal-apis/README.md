# Rate limiting com Minimal APIs

Exemplo de rate limiting por IP usando o middleware nativo do .NET 8 (`AddRateLimiter`), com janela fixa, resposta 429 e cabeçalho `Retry-After`.

## Rodando

```bash
docker compose up --build
```

A API sobe em http://localhost:8080. Chame `/` mais de 5 vezes em 10 segundos e veja o 429. O `/health` não tem limite.

## Testes

```bash
docker compose run --rm tests
```

Post: https://programero.blogspot.com/2026/11/rate-limiting-minimal-apis.html
