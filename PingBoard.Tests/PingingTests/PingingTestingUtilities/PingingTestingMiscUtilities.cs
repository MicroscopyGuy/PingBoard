namespace PingBoard.Tests.PingingTests;

public static class PingingTestingMiscUtilities {
    public static void AssertAllProperLength<A, B, C, D, E>(List<A> val1, List<B> val2, List<C> val3, 
                                                List<D> val4 , List<E> val5, int length) {
        Assert.Equal(length, val1.Count);
        Assert.Equal(length, val2.Count);
        Assert.Equal(length, val3.Count);
        Assert.Equal(length, val4.Count);
        Assert.Equal(length, val5.Count);
    }
}