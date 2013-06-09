--for i = 1, ExplosionParticleEmitter:ParticleNum do
for i = 1, ExplosionParticleEmitter.ParticleNum do
    --duration = (Random:NextDouble(0, 20)) / 10.0 + 2;
    duration = (Random:NextDouble()* 20) / 10.0 + 2;
    x = (Random:NextDouble() - 0.5) * 1.5;
    y = (Random:NextDouble() - 0.5) * 1.5;
    z = (Random:NextDouble() - 0.5) * 1.5;
    
    s = Random:NextDouble() + 0.5;
    
    --d = new Vector3(x, y, z);-- luaでは数式内で構造体を作ろうとするとエラーになるようだ。
    d = ExplosionParticleEmitter:CreateVector(x, y, z);
    --[[d = Vector3;
    d:X = x;--  function arguments expected near '='
    d:Y = y;
    d:Z = z;]]
    
    
    --[[direction = Vector3:Normalize(
    d *((Random:NextDouble() * 3) + 6));--  attempt to perform arithmetic on global 'd' (a userdata value)
    ]]
    direction = ExplosionParticleEmitter:NormalizeVector(
        --Vector3:Multiply( d, (Random:NextDouble() * 3) + 6)
        ExplosionParticleEmitter:MultiplyVector(d, (Random:NextDouble() * 3) + 6)
    );
    
    --offset = new Vector3(0, -2, 0);
    offset = ExplosionParticleEmitter:CreateVector(0, -2, 0);
    ExplosionParticleEmitter:AddParticle(
        ExplosionParticleEmitter:AddVector(ExplosionParticleEmitter.Position, offset), direction, s
    );
end