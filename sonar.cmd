set SONAR_RUNNER_OPTS=-Xmx512m -XX:MaxPermSize=128m
call .\build BuildSolutions
call .\nuget-packages\SonarRunner.2.3\bin\sonar-runner.bat -Dsonar.jdbc.password=%SONAR_PASSWORD%