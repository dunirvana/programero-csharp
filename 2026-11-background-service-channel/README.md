# Fila em memória com BackgroundService e Channel

Exemplo de processamento em segundo plano no .NET 8: a API recebe o pedido, coloca o job numa fila (`Channel<T>` limitada) e responde 202. Um `BackgroundService` consome a fila e atualiza o status do job.

## Rodando

```bash
docker compose up --build
```

A API sobe em http://localhost:8080.

```bash
curl -X POST http://localhost:8080/jobs -H "Content-Type: application/json" -d '{"name":"report"}'
curl http://localhost:8080/jobs/<id>
```

Se a fila encher, o POST responde 503. Um job com o nome `fail` simula uma falha, e o worker segue vivo.

## Testes

```bash
docker compose run --rm tests
```

Post: https://programero.blogspot.com/2026/11/background-service-channel.html
