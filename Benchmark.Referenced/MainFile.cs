public static class MainFile
{
    [Monitor(InterceptionMode: InterceptionMode.ExecutionTime, LogDestination: LogDestination.Console)]
    public static int Work() {
        int i = 0;int res = 0;
        while(i++<1000) {res += i;}
        return res;
    }
}