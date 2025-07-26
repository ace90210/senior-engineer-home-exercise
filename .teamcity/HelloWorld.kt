import jetbrains.buildServer.configs.kotlin.v2025_07.*
import jetbrains.buildServer.configs.kotlin.v2025_07.buildSteps.script

object HelloWorld : BuildType({
    name = "Hello World Build"

    steps {
        script {
            name = "Say Hello"
            scriptContent = "echo Hello, World!"
        }
    }
})
