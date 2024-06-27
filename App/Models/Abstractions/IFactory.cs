namespace App.Models.Abstractions {
	public interface IFactory<T> {
		T Create();
	}
}