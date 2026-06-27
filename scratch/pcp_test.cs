using System;
unsafe class T {
    static void Run(int* p, int n, long k, int* res) {
        for (int i=0;i<n;i++) res[i]=-1;
        for (int start=0;start<n;start++){
            if (res[start]!=-1) continue;
            int len=0,node=start; do{node=p[node];len++;}while(node!=start);
            int shift=(int)(k%len); if(shift<0)shift+=len;
            int target=start; for(int s=0;s<shift;s++)target=p[target];
            int cur=start; for(int i=0;i<len;i++){int nx=p[cur];res[cur]=target;cur=nx;target=p[target];}
        }
    }
    // brute: apply p k times
    static int[] Brute(int[] p,long k){int n=p.Length;var r=new int[n];for(int i=0;i<n;i++){int c=i;for(long t=0;t<k;t++)c=p[c];r[i]=c;}return r;}
    static void Main(){
        var rnd=new Random(7);
        for(int trial=0;trial<2000;trial++){
            int n=1+rnd.Next(8);
            var perm=new int[n]; for(int i=0;i<n;i++)perm[i]=i;
            for(int i=n-1;i>0;i--){int j=rnd.Next(i+1);(perm[i],perm[j])=(perm[j],perm[i]);}
            long k=rnd.Next(0,40);
            var res=new int[n];
            fixed(int* pp=perm) fixed(int* rr=res) Run(pp,n,k,rr);
            var br=Brute(perm,k);
            for(int i=0;i<n;i++) if(res[i]!=br[i]){Console.WriteLine($"FAIL trial{trial} n{n} k{k} i{i} got{res[i]} exp{br[i]}");return;}
        }
        // big k via mod equivalence: huge k
        for(int trial=0;trial<500;trial++){
            int n=1+rnd.Next(8); var perm=new int[n]; for(int i=0;i<n;i++)perm[i]=i;
            for(int i=n-1;i>0;i--){int j=rnd.Next(i+1);(perm[i],perm[j])=(perm[j],perm[i]);}
            long big=(long)rnd.Next(1000000)*1000000L+rnd.Next(1000000);
            // lcm of cycle lengths bounded small; reduce by walking small equivalent
            long small=big % 2520; // 2520=lcm(1..9) covers n<=9
            var res=new int[n]; fixed(int* pp=perm) fixed(int* rr=res) Run(pp,n,big,rr);
            var br=Brute(perm,small);
            for(int i=0;i<n;i++) if(res[i]!=br[i]){Console.WriteLine($"FAILBIG n{n} big{big} i{i} got{res[i]} exp{br[i]}");return;}
        }
        Console.WriteLine("ALL PASS");
    }
}
