--for i = 1, 10 do
--end

--[[for (int i = 0; i < ParticleNum; i++) {
    float duration = (float)(rand.Next(0, 20)) / 10f + 2;
    float x = Level2.NextDouble(rand, -1, 1);
    float y = Level2.NextDouble(rand, -1, 1);
    float z = Level2.NextDouble(rand, -1, 1); float x = ((float)rand.NextDouble() - 0.5f) * 1.5f;

    //float s = (float)rand.NextDouble() + 1.0f;
    float s = (float)rand.NextDouble() + 1.0f;
    Vector3 direction = Vector3.Normalize(
        new Vector3(x, y, z)) *
        (((float)rand.NextDouble() * 3f) + 6f);

    AddParticle(Position + new Vector3(0, -2, 0), direction, s);
}]]

for i = 1, ExplosionParticleEmitter:ParticleNum do
    duration = (rand.Next(0, 20)) / 10f + 2;
    x = (Random.NextDouble() - 0.5f) * 1.5f;
    y = (Random.NextDouble() - 0.5f) * 1.5f;
    z = (Random.NextDouble() - 0.5f) * 1.5f;
    
    s = Random.NextDouble() + 1.0f;
    direction = Vector3:Normalize(
        new Vector3(x, y, z)) *
        ((Random.NextDouble() * 3f) + 6f);

    AddParticle(Position + new Vector3(0, -2, 0), direction, s);
    
end