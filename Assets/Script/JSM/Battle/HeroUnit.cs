public class HeroUnit : Unit
{
    public override void Initialize()
    {
        base.Initialize();
        if(stats.SkillID!=null&& stats.SkillID.Count>=2) SkillManager.Instance.UseSkill(stats.SkillID[1], this.isEnemy);
    }
}
