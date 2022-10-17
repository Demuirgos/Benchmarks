# Benchmarks
a bunch of experiments and benchmarks

# Usage : 
```
[MonitorExecutionTime]
public static int SumSync(int n, int k) {
    int res = 0;
    for(int i = n; i < k; i++){
        res += i;
    }
    return res;
}
```
#Output : 
```
TestFunc.SumSync executed in 3 ms
```
