namespace DAL.Mapper
{
    public interface IMapper<TI, TO>
    {
        TO Create(TI target);

        void Update(TI from, TO to);
    }
}
