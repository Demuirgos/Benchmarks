using static TestFunc;

SumSync(0, 100);
await SumAsync(0, 100);

class TestFunc {

    [MonitorExecutionTime]
    public static int SumSync(int n, int k) {
        int res = 0;
        for(int i = n; i < k; i++){
            res += i;
        }
        return res;
    }

    [MonitorExecutionTime]
    public static async Task<int> SumAsync(int n, int k) {
        int res = 0;
        for(int i = n; i < k; i++){
            await Task.Delay(10);
            res += i;
        }
        return res;
    }
}