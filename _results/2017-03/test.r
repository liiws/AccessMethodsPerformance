testRaw1 <- "items,manual,reflection,expression
20000,4692,5615,4342
10000,1769,2195,1931
5000,502,679,583
2500,204,282,304
1250,93,139,174
625,48,71,129
312,23,34,99
156,10,16,85
78,5,7,77
39,2,3,75
19,1,2,73
9,0,0,75
"

testRaw2 <- "items,manual,reflection,expression
20000,4584,5999,4403
10000,1780,2209,1994
5000,489,676,587
2500,198,292,306
1250,93,140,172
625,46,73,128
312,23,35,99
156,10,16,84
78,5,8,79
39,2,3,74
19,1,2,73
9,0,0,71
"

testRaw3 <- "items,manual,reflection,expression
20000,4601,5578,4386
10000,1757,2333,1977
5000,495,674,578
2500,198,311,303
1250,98,137,176
625,49,72,129
312,23,33,101
156,10,16,85
78,5,7,79
39,3,4,75
19,1,1,72
9,0,0,74
"

# read data
testData1 <- read.csv(text = testRaw1)
testData2 <- read.csv(text = testRaw2)
testData3 <- read.csv(text = testRaw3)

# avg
avg <- (testData1+testData2+testData3)/3
# make real time, not for all iteratons - divide results except first column by number of iterations
iterations <- 200
row1 <- testData1[1:1]
rowOthers <- testData1[-(1:1)]/iterations
avg <- cbind(row1, rowOthers)


xValues = c(1:nrow(avg))
xLabels <- rev(c(t(avg[1])))
yManual <- rev(c(t(avg[2])))
yReflection <- rev(c(t(avg[3])))
yExpression <- rev(c(t(avg[4])))

createPng <- function(width, height, xLim, yLim, name)
{
  png(
    file = paste0(dirname(sys.frame(1)$ofile), "/", name),
    width = width, height = height, type = "cairo")
  
  colors <- rainbow(3)
  plot(0, 0, main = "Access Methods Performance", xlab = "Number of ListViewItem classes created", ylab = "Time, ms",
       type = "n", xlim = xLim, ylim = yLim, xaxt = "n" )
  axis(1, at = xValues, labels = xLabels )
  lines(xValues, yManual, type = "l", col = colors[1])
  lines(xValues, yReflection, type = "l", col = colors[2])
  lines(xValues, yExpression, type = "l", col = colors[3])
  legend("topleft", legend = names(avg)[-(1:1)], col = colors, pch = 1)
  
  dev.off()
}

createPng(700, 500, c(0,nrow(avg)), c(0,28), "whole.png")
createPng(700, 500, c(0,9+1), c(0,5), "low.png")
