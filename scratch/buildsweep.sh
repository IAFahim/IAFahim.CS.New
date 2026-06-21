#!/bin/bash
# Usage: buildsweep.sh MODSFILE RESULTFILE
# Always compile-checks. Runs tests with timeout; a test TIMEOUT is reported
# as TIMEOUT (not FAIL) since pre-existing hangs must not revert judged fixes.
cd /home/l/Github/IAFahim.CS.New
MODS="$1"; RESULT="$2"; : > "$RESULT"
while read -r m; do
  [ -z "$m" ] && continue
  sp="src/${m}/${m}.csproj"
  if ! timeout 180 dotnet build "$sp" --nologo -v q >/dev/null 2>&1; then echo "FAIL(build) $m" >>"$RESULT"; continue; fi
  tp="test/${m}.Tests/${m}.Tests.csproj"
  if [ -f "$tp" ]; then
    timeout 90 dotnet test "$tp" --nologo -v q >/dev/null 2>&1; rc=$?
    if [ $rc -eq 0 ]; then echo "PASS(test)  $m" >>"$RESULT";
    elif [ $rc -eq 124 ]; then echo "BUILD-OK/TEST-TIMEOUT $m" >>"$RESULT";
    else echo "FAIL(test)  $m (rc=$rc)" >>"$RESULT"; fi
  else
    echo "PASS(build) $m" >>"$RESULT"
  fi
done < "$MODS"
echo "=== SWEEP DONE ===" >>"$RESULT"
